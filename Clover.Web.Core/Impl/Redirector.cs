using System;
using System.Web;
using StructureMap;

namespace Clover.Web.Core.Impl
{
    [Pluggable("Default")]
    public class Redirector : IRedirector
    {
        
        public void GoToGroupsManageGroup(int GroupID)
        {
            Redirect("~/Groups/ManageGroup.aspx?GroupID=" + GroupID.ToString());
        }
        
        public void GoToGroupsMembers(int GroupID, int PageNumber)
        {
            Redirect("~/Groups/Members.aspx?GroupID=" + GroupID.ToString() + "&PageNumber=" + PageNumber.ToString());
        }
        
        public void GoToGroupsViewGroup(int GroupID)
        {
            Redirect("~/Groups/ViewGroup.aspx?GroupID=" + GroupID.ToString());
        }
        
        public void GoToGroupsMembers(int GroupID)
        {
            Redirect("~/Groups/Members.aspx?GroupID=" + GroupID.ToString());
        }
        
        public void GoToGroupsViewGroup(string GroupPageName)
        {
            Redirect("~/Groups/" + GroupPageName + ".aspx");
        }
        
        public void GoToForumsViewPost(string ForumPageName, string CategoryPageName, string PostPageName)
        {
            Redirect("~/Forums/" + CategoryPageName + "/" + ForumPageName + "/" + PostPageName + ".aspx");
        }
        
        public void GoToForumsForumView(string ForumPageName, string CategoryPageName)
        {
            Redirect("~/Forums/" + CategoryPageName + "/" + ForumPageName + ".aspx");
        }
        public void GoToBlogsPostEdit(Int64 BlogID)
        {
            Redirect("~/Blogs/Post.aspx?BlogID=" + BlogID.ToString());
        }
        public void GoToPhotosMyPhotos()
        {
            Redirect("~/Photos/MyPhotos.aspx");
        }
        public void GoToPhotosEditAlbum(Int64 AlbumID)
        {
            Redirect("~/Photos/EditAlbum.aspx?AlbumID=" + AlbumID.ToString());    
        }

        public void GoToPhotosEditPhotos(Int64 AlbumID)
        {
            Redirect("~/Photos/EditPhotos.aspx?AlbumID=" + AlbumID.ToString());    
        }

        public void GoToPhotosViewAlbum(Int64 AlbumID)
        {
            Redirect("~/Photos/ViewAlbum.aspx?AlbumID=" + AlbumID.ToString());
        }
        public void GoToPhotos()
        {
            Redirect("~/Photos/Default.aspx");
        }
        public void GoToPhotosAddPhotos(Int64 AlbumID)
        {
            Redirect("~/Photos/AddPhotos.aspx?AlbumID=" + AlbumID.ToString());
        }
        public void GoToMailNewMessage(Int32 MessageID)
        {
            Redirect("~/Mail/NewMessage.aspx?MessageID=" + MessageID.ToString());
        }

        public void GoToProfilesProfile()
        {
            Redirect("~/Profiles/Profile.aspx");
        }
        public void GoToProfilesDefault()
        {
            Redirect("~/Profiles/Default.aspx");
        }
        public void GoToProfilesManageProfile()
        {
            Redirect("~/Profiles/ManageProfile.aspx");
        }
        public void GoToAccountAccessDenied()
        {
            Redirect("~/Accounts/AccessDenied.aspx");    
        }
        public void GoToAccountRecoverPasswordPage()
        {
            Redirect("~/Accounts/RecoverPassword.aspx");
        }
        public void GoToAccountEditAccountPage()
        {
            Redirect("~/Accounts/EditAccount.aspx");    
        }

        
        public void GoToProfilesStatusUpdates()
        {
            Redirect("~/Profiles/StatusUpdates.aspx");
        }

        
        public void GoToProfilesStatusUpdates(Int32 AccountID)
        {
            Redirect("~/Profiles/StatusUpdates.aspx?" + AccountID.ToString());
        }

        
        public void GoToSearch(string SearchText)
        {
           Redirect("~/Search.aspx?s=" + SearchText); 
        }

        public void GoToAccountLoginPage(string FriendInvitationKey)
        {
            Redirect("~/Accounts/Login.aspx?InvitationKey=" + FriendInvitationKey);
        }
        
        public void GoToAccountLoginPage()
        {
            Redirect("~/Accounts/Login.aspx");
        }
        
        public void GoToFriendsInviteFriends(Int32 AccoundIdToInvite)
        {
            Redirect("~/Friends/InviteFriends.aspx?AccountIdToInvite=" + AccoundIdToInvite.ToString());
        }

        
        public void GoToAccountRegisterPage(string FriendInvitationKey)
        {
            Redirect("~/Accounts/Register.aspx?InvitationKey=" + FriendInvitationKey);
        }

        public void GoToAccountRegisterPage()
        {
            Redirect("~/Accounts/Register.aspx");
        }
        public void GoToHomePage()
        {
            Redirect("~/Default.aspx");
        }
        public void GoToErrorPage()
        {
            Redirect("~/Error.aspx");
        }
        private void Redirect(string path)
        {
            HttpContext.Current.Response.Redirect(path);
        }
    }
}
