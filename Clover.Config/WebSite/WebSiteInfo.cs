using System;
using System.Xml.Serialization;
using Clover.Core.Configuration;

namespace Clover.Config.WebSiteSetting
{
    [XmlRoot("WebSiteConfig")]
    
    
    
    [Serializable]
    public class WebSiteConfigInfo : IConfigInfo
    {
        #region 私有字段
        private string m_webappnme = "缤纷影视系统"; 
        private string m_weburl = ""; 
        private int m_licensed = 1; 
        private string m_icp = ""; 
        private int m_closed = 0; 
        private string m_closedreason = "";
        private string m_linktext = "";


        private string m_passwordkey = "1234567890"; 
        private string m_ipdenyaccess = ""; 
        private int m_ipdenyaccesstype = 0; 
        private string m_ipdenyaccesstip ="您被禁止访问本站.";  
        private string m_adminipaccess = ""; 
        private string m_adminipaccesstip = "您没有权限进入后台管理";
        private string m_resipaccess = ""; 
        private string m_resipaccesstip = "您没有权限访问该资源";
        private string m_visitbanperiods = ""; 
        private string m_visitbanperiodstip = "工作时间不允许访问本站.";
        private string m_cookiedomain = "";
        private int m_templateid = 1; 
        private string m_seotitle = ""; 
        private string m_seokeywords = ""; 
        private string m_seodescription = ""; 
        private string m_seohead = "<meta name=\"generator\" content=\"bfvod3\" />"; 
        private string m_copyright = "";    
        private string m_notices = "暂无公告";
        private int m_cachetime = 0; 
        private string m_cachepage = "index";   
        private int m_nocacheheaders = 0; 
        private int m_aspxrewrite = 1; 

        private int m_allowuserregister = 1;    
        private int m_logintolook = 0;          
        private int m_tofilmcoin = 10;      
        private int m_todays = 1;         
        private int m_newuserstatus = 1;    
        private int m_downloadfilmcoinrate = 2;     
        private int m_downloadspeed = 1024; 

        private int m_gbneedauditing = 0;   
        private string m_replyformat = "";  
        private int m_plneedauditing = 0;   
        private int m_usesimplenotice = 0;  

        private string m_company = ""; 
        private string m_supplier = ""; 
        private string m_addminmail = ""; 
        private string m_servicemail = ""; 
        private string m_positionmail = ""; 

        private int m_webplayspeed = 50; 
        private int m_webdownloadspeed = 50; 

        private string m_systemno = "";

        private int m_timeconsuming = 5; 

        private string m_badwords = ""; 
        #endregion

        #region 属性
        
        [XmlElement()]
        
        
        
        public string BadWords
        {
            get { return m_badwords; }
            set { m_badwords = value; }
        }
        [XmlElement()]
        
        
        
        public int TimeConsuming
        {
            get { return m_timeconsuming; }
            set { m_timeconsuming = value; }
        }
     
        [XmlElement()]
        
        
        
        public string WebAppName
        {
            get { return m_webappnme; }
            set { m_webappnme = value; }
        }
     
        [XmlElement()]
        
        
        
        public string Company
        {
            get { return m_company; }
            set { m_company = value; }
        }

        [XmlElement()]
        
        
        
        public string Supplier
        {
            get { return m_supplier; }
            set { m_supplier = value; }
        }



        [XmlElement()]
        
        
        
        public string Weburl
        {
            get { return m_weburl; }
            set { m_weburl = value; }
        }

        [XmlElement()]
        
        
        
        public int Licensed
        {
            get { return m_licensed; }
            set { m_licensed = value; }
        }

        [XmlElement()]
        
        
        
        public string AdminEmail
        {
            get { return m_addminmail; }
            set { m_addminmail = value; }
        }

        [XmlElement()]
        
        
        
        public string ServicesEmail
        {
            get { return m_servicemail; }
            set { m_servicemail = value; }
        }

        [XmlElement()]
        
        
        
        public string PositionEmail
        {
            get { return m_positionmail; }
            set { m_positionmail = value; }
        }

        [XmlElement()]
        
        
        
        public string Icp
        {
            get { return m_icp; }
            set { m_icp = value; }
        }

        [XmlElement()]
        
        
        
        public int Closed
        {
            get { return m_closed; }
            set { m_closed = value; }
        }

        [XmlElement()]
        
        
        
        public string Closedreason
        {
            get { return m_closedreason; }
            set { m_closedreason = value; }
        }



        [XmlElement()]
        
        
        
        public string Linktext
        {
            get { return m_linktext; }
            set { m_linktext = value; }
        }

        [XmlElement()]
        
        
        
        public string Passwordkey
        {
            get { return m_passwordkey; }
            set { m_passwordkey = value; }
        }

        [XmlElement()]
        
        
        
        public string CookieDomain
        {
            get { return m_cookiedomain; }
            set { m_cookiedomain = value; }
        }

        [XmlElement()]
        
        
        
        public string Ipdenyaccess
        {
            get { return m_ipdenyaccess; }
            set { m_ipdenyaccess = value; }
        }

        [XmlElement()]
        
        
        
        public int IpdenyaccessType
        {
            get { return m_ipdenyaccesstype; }
            set { m_ipdenyaccesstype = value; }
        }

        [XmlElement()]
        
        
        
        public string IpdenyaccessTip
        {
            get { return this.m_ipdenyaccesstip; }
            set { this.m_ipdenyaccesstip = value; }
        }

        [XmlElement()]
        
        
        
        public string Adminipaccess
        {
            get { return m_adminipaccess; }
            set { m_adminipaccess = value; }
        }

        [XmlElement()]
        
        
        
        public string AdminipaccessTip
        {
            get { return m_adminipaccesstip; }
            set { m_adminipaccesstip = value; }
        }

        [XmlElement()]
        
        
        
        public string Resipaccess
        {
            get { return m_resipaccess; }
            set { m_resipaccess = value; }
        }

        [XmlElement()]
        
        
        
        public string ResipaccessTip
        {
            get { return m_resipaccesstip; }
            set { m_resipaccesstip = value; }
        }

        [XmlElement()]
        
        
        
        public string Visitbanperiods
        {
            get { return m_visitbanperiods; }
            set { m_visitbanperiods = value; }
        }

        [XmlElement()]
        
        
        
        public string VisitbanperiodsTip
        {
            get { return m_visitbanperiodstip; }
            set { m_visitbanperiodstip = value; }
        }

        [XmlElement()]
        
        
        
        public int Templateid
        {
            get { return m_templateid; }
            set { m_templateid = value; }
        }


        [XmlElement()]
        
        
        
        public int CacheTime
        {
            get { return m_cachetime; }
            set { m_cachetime = value; }
        }

        [XmlElement()]
        
        
        
        public string CachePage
        {
            get { return this.m_cachepage; }
            set { this.m_cachepage = value; }
        }


        [XmlElement()]
        
        
        
        public string Seotitle
        {
            get { return m_seotitle; }
            set { m_seotitle = value; }
        }

        [XmlElement()]
        
        
        
        public string Seokeywords
        {
            get { return m_seokeywords; }
            set { m_seokeywords = value; }
        }

        [XmlElement()]
        
        
        
        public string Seodescription
        {
            get { return m_seodescription; }
            set { m_seodescription = value; }
        }

        [XmlElement()]
        
        
        
        public string Seohead
        {
            get { return m_seohead; }
            set { m_seohead = value; }
        }

        [XmlElement()]
        
        
        
        public int Nocacheheaders
        {
            get { return m_nocacheheaders; }
            set { m_nocacheheaders = value; }
        }


        [XmlElement()]     
        
        
        
        public string Copyright
        {
            get { return m_copyright; }
            set { m_copyright = value; }
        }

        [XmlElement()]
        
        
        
        public string Notice
        {
            get { return m_notices; }
            set { m_notices = value; }
        }

        [XmlElement()]
        
        
        
        public int Aspxrewrite
        {
            get { return m_aspxrewrite; }
            set { m_aspxrewrite = value; }
        }

        [XmlElement()]
        
        
        
        public int AllowUserRegister
        {
            get { return this.m_allowuserregister; }
            set { this.m_allowuserregister = value; }
        }

        [XmlElement()]
        
        
        
        public int LoginToLook
        {
            get { return this.m_logintolook; }
            set { this.m_logintolook = value; }
        }

        [XmlElement()]
        
        
        
        public int ToFilmCoin
        {
            get { return this.m_tofilmcoin; }
            set { this.m_tofilmcoin = value; }
        }


        [XmlElement()]
        
        
        
        public int ToDays
        {
            get { return this.m_todays; }
            set { this.m_todays = value; }
        }

        [XmlElement()]
        
        
        
        public int NewUserStatus
        {
            get { return this.m_newuserstatus; }
            set { this.m_newuserstatus = value; }
        }

        [XmlElement()]
        
        
        
        public int DownloadFilmCoinRate
        {
            get { return this.m_downloadfilmcoinrate; }
            set { this.m_downloadfilmcoinrate = value; }
        }

        [XmlElement()]
        
        
        
        public int DownloadSpeed
        {
            get { return this.m_downloadspeed; }
            set { this.m_downloadspeed = value; }
        }

        [XmlElement()]
        
        
        
        public int GBNeedAuditing
        {
            get { return this.m_gbneedauditing; }
            set { this.m_gbneedauditing = value; }
        }

        [XmlElement()]
        
        
        
        public string ReplyFormat
        {
            get { return this.m_replyformat; }
            set { this.m_replyformat = value; }
        }

        [XmlElement()]
        
        
        
        public int PlNeedAuditing
        {
            get { return this.m_plneedauditing; }
            set { this.m_plneedauditing = value; }
        }

        [XmlElement()]
        
        
        
        public int UseSimpleNotice
        {
            get { return this.m_usesimplenotice; }
            set { this.m_usesimplenotice = value; }
        }

        [XmlElement()]
        
        
        
        public int WebPlaySpeed
        {
            get { return m_webplayspeed; }
            set { m_webplayspeed = value; }
        }

        [XmlElement()]
        
        
        
        public int WebDownloadSpeed
        {
            get { return m_webdownloadspeed; }
            set { m_webdownloadspeed = value; }
        }


        int m_adminpagesize = 20;

        [XmlElement()]
        
        
        
        public int AdminPageSize
        {
            get { return m_adminpagesize; }
            set { m_adminpagesize = value; }
        }

        [XmlElement()]
        
        
        
        public string SystemNo
        {
            get { return m_systemno; }
            set { m_systemno = value; }
        }


        [XmlElement()]
        
        
        
        public bool UsePassWordStrategy
        {
            get;
            set;
        }

        
        string m_passwordregex = "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@_\\!\\.])(?!.*[\\'\\\"\\s\\*]).{6,18}$";
        [XmlElement()]
        
        
        
        public string PasswordRegex
        {
            get { return m_passwordregex; }
            set { m_passwordregex = value; }
        }


        int m_changepasswordperiod = 180;
        [XmlElement()]
        
        
        
        public int ChangePasswordPeriod
        {
            get { return m_changepasswordperiod; }
            set { m_changepasswordperiod = value; }
        }


        string m_passwordnotmatchmessage = "密码长度需要6-18位，至少包含3种类型：字母（大小写）,数字，特殊符号(@_!.)";
        [XmlElement()]
        
        
        
        public string PasswordNotMatchMessage
        {
            get{return m_passwordnotmatchmessage;}
            set { m_passwordnotmatchmessage = value; }
        }

        [XmlElement()]
        
        
        
        public bool TaskAutomation
        {
            get;
            set;
        }
        #endregion
    }
}
