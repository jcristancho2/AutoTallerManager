using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities
{
    public class ActionType : BaseEntity
    {
        public string? ActionName { get; set; }

        private ActionType() { }

        public ActionType(string actionName)
        {
            ActionName = actionName;
        }
    }
}