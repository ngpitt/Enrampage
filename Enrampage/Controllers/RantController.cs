using Enrampage.Extensions;
using Enrampage.Helpers;
using Enrampage.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace Enrampage.Controllers
{
    public class RantController : BaseController
    {
        [HttpPost]
        public JsonResult GetRants(PageRequest Page)
        {
            try
            {
                var rantResponses = new List<RantResponse>();

                using (var context = new EnrampageEntities())
                {
                    IQueryable<Rant> rants = context.Rants;

                    if (Page.Tags != null)
                    {
                        rants = rants.Where(r => r.Tags.Select(t => t.Text).Intersect(Page.Tags).Any());
                    }

                    foreach (var rant in rants.OrderByDescending(r => r.Timestamp).Skip(Page.Number * 10).Take(10))
                    {
                        var reportState = ReportState.Reportable;

                        if (!Request.IsAuthenticated)
                        {
                            reportState = ReportState.None;
                        }
                        else if (rant.UserId == CurrentUser.UserId() || CurrentUser.Admin())
                        {
                            reportState = ReportState.Removable;
                        }
                        else if (rant.Reports.Any(r => r.UserId == CurrentUser.UserId()))
                        {
                            reportState = ReportState.AlreadyReported;
                        }

                        rantResponses.Add(RantResponse.FromRant(rant, reportState));
                    }
                }

                return Json(new ApiResponse(true, "Rant listing successful.", rantResponses));
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                return Json(new ApiResponse(false, "Failed to list rants."));
            }
        }

        [HttpPost]
        public JsonResult GetTags()
        {
            try
            {
                using (var context = new EnrampageEntities())
                {
                    return Json(new ApiResponse(true, "Tag listing successful.",
                        context.Tags.Where(t => !t.User.Banned).Select(t => t.Text).ToArray()));
                }
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                return Json(new ApiResponse(false, "Failed to list tags."));
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult PostRant(PostRequest Rant)
        {
            try
            {
                var rant = new Rant
                {
                    UserId = CurrentUser.UserId(),
                    Timestamp = DateTime.Now,
                    Text = Rant.Text.ToUpper()
                };

                if (Rant.Tags == null)
                {
                    return Json(new ApiResponse(false, "At least one tag is required."));
                }

                if (Rant.Tags.Any(t => string.IsNullOrWhiteSpace(t)))
                {
                    return Json(new ApiResponse(false, "You cannot submit an empty tag."));
                }

                if (string.IsNullOrWhiteSpace(Rant.Text))
                {
                    return Json(new ApiResponse(false, "You cannot submit an empty rant."));
                }

                using (var context = new EnrampageEntities())
                {
                    context.Tags.AddRange(
                        Rant.Tags.Except(context.Tags.Where(t => !t.User.Banned).Select(t => t.Text))
                        .Select(t => new Tag { UserId = rant.UserId, Text = t }));
                    context.SaveChanges();

                    foreach (var tag in Rant.Tags)
                    {
                        rant.Tags.Add(context.Tags.First(t => !t.User.Banned && t.Text == tag));
                    }

                    context.Rants.Add(rant);
                    context.SaveChanges();
                }

                return Json(new ApiResponse(true, "Posted rant successfully.", RantResponse.FromRant(rant, ReportState.Removable)));
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                return Json(new ApiResponse(false, "Failed to post rant."));
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult RemoveRant(int Id)
        {
            var result = (ApiResponse)(RemoveRant(new RemoveRequest { Id = Id }).Data);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveRant(RemoveRequest Request)
        {
            try
            {
                using (var context = new EnrampageEntities())
                {
                    var rant = context.Rants.FirstOrDefault(r => r.Id == Request.Id);

                    if (rant == null)
                    {
                        return Json(new ApiResponse(false, "Rant not found."));
                    }

                    if (rant.UserId != CurrentUser.UserId() && !CurrentUser.Admin())
                    {
                        return Json(new ApiResponse(false, "Rant not post by you."));
                    }

                    rant.Tags.Clear();
                    context.Reports.RemoveRange(rant.Reports);
                    context.Rants.Remove(rant);
                    context.SaveChanges();
                }

                return Json(new ApiResponse(true, "Rant removed successfully."));
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                return Json(new ApiResponse(false, "Failed to remove rant."));
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult RemoveTag(int Id)
        {
            var result = (ApiResponse)(RemoveTag(new RemoveRequest { Id = Id }).Data);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveTag(RemoveRequest Request)
        {
            try
            {
                using (var context = new EnrampageEntities())
                {
                    var tag = context.Tags.FirstOrDefault(t => t.Id == Request.Id);

                    if (tag == null)
                    {
                        return Json(new ApiResponse(false, "Tag not found."));
                    }

                    if (tag.UserId != CurrentUser.UserId() && !CurrentUser.Admin())
                    {
                        return Json(new ApiResponse(false, "Tag not created by you."));
                    }

                    tag.Rants.Clear();
                    context.Tags.Remove(tag);
                    context.SaveChanges();
                }

                return Json(new ApiResponse(true, "Tag removed successfully."));
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                return Json(new ApiResponse(false, "Failed to remove tag."));
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult ReportRant(ReportRequest Report)
        {
            try
            {
                var report = new Report
                {
                    UserId = CurrentUser.UserId(),
                    Timestamp = DateTime.Now,
                    Text = Report.Text
                };

                if (string.IsNullOrWhiteSpace(report.Text))
                {
                    return Json(new ApiResponse(false, "You cannot sumbit an empty report."));
                }

                using (var context = new EnrampageEntities())
                {
                    var rant = context.Rants.FirstOrDefault(r => r.Id == Report.Id);

                    if (rant == null)
                    {
                        return Json(new ApiResponse(false, "Rant not found."));
                    }

                    if (rant.UserId == report.UserId)
                    {
                        return Json(new ApiResponse(false, "Rant posted by you."));
                    }

                    if (context.Reports.Any(r => r.UserId == report.UserId && r.RantId == rant.Id))
                    {
                        return Json(new ApiResponse(false, "You have already reported this rant."));
                    }

                    report.Rant = rant;
                    context.Reports.Add(report);
                    context.SaveChanges();

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(ConfigurationManager.AppSettings["ReportFrom"]);

                        foreach (var email in context.Users.Where(u => u.Admin).Select(u => u.Email))
                        {
                            message.Bcc.Add(email);
                        }

                        message.Subject = "Rant Report";
                        message.Body = string.Format("Rant: {0}\n\nReport: {1}\n\nRemove Rant: {2}\nBan User: {3}",
                            report.Rant.Text,
                            report.Text,
                            Url.Action("Login", "Account", new { ReturnUrl = Url.Action("RemoveRant", "Rant", new { id = report.Rant.Id }) }, Request.Url.Scheme),
                            Url.Action("Login", "Account", new { ReturnUrl = Url.Action("BanUser", "Account", new { id = report.Rant.UserId }) }, Request.Url.Scheme));

                        using (var smtpClient = new SmtpClient())
                        {
                            smtpClient.Send(message);
                        }
                    }
                }

                return Json(new ApiResponse(true, "Report submitted successfully."));
            }
            catch (Exception Ex)
            {
                LogHelper.Log(Ex);
                return Json(new ApiResponse(false, "Faild to submit report."));
            }
        }
    }
}
