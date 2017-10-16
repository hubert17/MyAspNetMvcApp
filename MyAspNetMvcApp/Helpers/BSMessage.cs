using System;

public static class BSMessage
{
    public const string TYPE = "MessageType";
    public const string ALERT = "MessageAlert";
    public const string PANEL = "MessagePanel";
    public const string DIALOGBOX = "MessageBox";
    public static class MessageType
    {
        public const string TYPE = "default";
        public const string PRIMARY = "primary";
        public const string INFO = "info";
        public const string SUCCESS = "success";
        public const string WARNING = "warning";
        public const string DANGER = "danger";
    }
}
