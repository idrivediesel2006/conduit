using Conduit.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class TagsRepository : ITagsRepository
    {
        private ConduitContext Context;
        private ILogger<TagsRepository> Logger;

        public string[] GetTags()
        {
            string[] tags = (from t in Context.Tags
                             select t.DisplayName).ToArray();
            return tags;
        }

        public TagsRepository(ConduitContext context, ILogger<TagsRepository> logger)
        {
            Context = context;
            Logger = logger;
        }
    }
}
