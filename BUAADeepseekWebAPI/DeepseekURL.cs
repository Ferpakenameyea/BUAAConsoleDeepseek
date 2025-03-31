using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUAADeepseekWebAPI
{
    internal static class DeepseekURL
    {
        public const string BASE = "https://chat.buaa.edu.cn";

        public const string SESSION_LIST = "/site/voom/session_list";

        public const string USER_INFO = "/site/user_info";

        public const string SESSION_DELETE = "/site/voom/session_delete";

        public const string NEW_SESSION_ID = "/site/voom/get_new_session_id";

        public const string SESSION_UPDATE = "/site/voom/session_update";

        public const string COMPOSE_CHAT = "/site/ai/compose_chat";
    }
}
