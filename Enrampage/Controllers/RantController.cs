using Enrampage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Enrampage.Controllers
{
    public class RantController : ApiController
    {
        [HttpPost]
        public ApiResponse GetRants(PageRequest Page)
        {
            try
            {
                using (var context = new EnrampageEntities())
                {
                    var rantResponses = new List<RantResponse>();
                    IQueryable<Rant> rants = context.Rants;

                    if (Page.Tags.Length > 0)
                    {
                        rants = rants.Where(x => x.Tags.Any(y => Page.Tags.Any(z => z == y.Text)));
                    }

                    foreach (var rant in rants.OrderByDescending(x => x.Created).Skip(Page.Number * 10).Take(10))
                    {
                        rantResponses.Add(RantResponse.FromRant(rant));
                    }

                    return new ApiResponse(true, "Rant listing successful.", rantResponses);
                }
            }
            catch (Exception Ex)
            {
                return new ApiResponse(false, "Failed to list rants.", null);
            }
        }

        [HttpPost]
        public ApiResponse PostRant(RantRequest Rant)
        {
            try
            {
                var rant = new Rant
                {
                    Created = DateTime.Now,
                    Text = Rant.Text.ToUpper()
                };

                using (var context = new EnrampageEntities())
                {
                    context.Tags.AddRange(Rant.Tags.Where(x => !context.Tags.Any(y => y.Text == x)).Select(x => new Tag { Text = x }));
                    context.SaveChanges();

                    foreach (var tag in context.Tags.Where(x => Rant.Tags.Any(y => y == x.Text)))
                    {
                        rant.Tags.Add(tag);
                    }

                    context.Rants.Add(rant);
                    context.SaveChanges();
                }

                return new ApiResponse(true, "Saved rant successfully.", RantResponse.FromRant(rant));
            }
            catch
            {
                return new ApiResponse(false, "Failed to save rant.", null);
            }
        }

        [HttpPost]
        public ApiResponse GetTags()
        {
            try
            {
                using (var context = new EnrampageEntities())
                {
                    return new ApiResponse(true, "Tag listing successful.", context.Tags.Select(x => x.Text).ToList());
                }
            }
            catch
            {
                return new ApiResponse(false, "Failed to list tags.", null);
            }
        }
    }
}
