using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


// import authorize untuk mencegah anonymous access / user yang tidak login
using Microsoft.AspNetCore.Authorization;


namespace AspNet.Pages
{
    
    //mencegah akses kepada user yang tidak login untuk mengakses page yang membutuhkan authorize
    [Authorize]
    public class AdminModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
