namespace DotNet.Push.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class PushProtocol
    {
        public string to
        {
            get; set;
        }
        public string priority
        {
            get; set;
        }
        public Notification notification
        {
            get; set;
        }
        public NotifiData data
        {
            get; set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Notification
    {
        public string title
        {
            get; set;
        }
        public string body
        {
            get; set;
        }
        public string click_action
        {
            get; set;
        }
        public string icon
        {
            get; set;
        }
        public string color
        {
            get; set;
        }
        public string badge
        {
            get; set;
        }
        public string tag
        {
            get; set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NotifiData
    {
        public int badge
        {
            get; set;
        }
        public string title
        {
            get; set;
        }
        public string text
        {
            get; set;
        }
        public SubData sub
        {
            get; set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubData
    {
        public string sub
        {
            get; set;
        }
    }
}