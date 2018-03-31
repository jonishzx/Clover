using System;
using System.Collections.Generic;
using System.Text;

namespace Clover.Web.Core
{
    public interface IFormToken
    {
        
        
        
        void CreateToken();

        
        
        
        void ClearToken();

        
        
        
        string GetFormToken();

        
        
        
        
        
        
        bool IsValidToken();
    }
}
