using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    public interface IInjectionManager
    {
        public void PersistChange(InjectionTemplate template);
        public IEnumerable<InjectionTemplate> Enumerate();
        public InjectionTemplate CreateNewTemplate(string prompt, string followInput, string? comment = null);
        public Task InjectToSession(InjectionTemplate template, SessionMeta session);
        public InjectionTemplate? GetInjectionTemplate(string id);
        public void CommentTemplate(InjectionTemplate template, string comment);
        public void DeleteTemplate(InjectionTemplate template);
        public bool TryDeleteTemplate(string id)
        {
            var template = GetInjectionTemplate(id);
            if (template == null)
            {
                return false;
            }

            DeleteTemplate(template);
            return true;
        }
    }
}
