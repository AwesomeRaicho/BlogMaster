using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Utilities
{
    public static class SlugGenerator
    {
        public static string GenerateSlug(string blogTitle)
        {

            

            blogTitle = blogTitle.Trim().ToLower();

            StringBuilder slugBuilder = new StringBuilder();

            bool wasPreviousCharacterHyphen = false;

            foreach (char c in blogTitle)
            {
                if(char.IsLetterOrDigit(c))
                {
                    slugBuilder.Append(c);
                    wasPreviousCharacterHyphen = false;
                }else
                {
                    if(!wasPreviousCharacterHyphen)
                    {
                        slugBuilder.Append("-");
                        wasPreviousCharacterHyphen = true;
                    }

                }
            }
            
            
            return slugBuilder.ToString().Trim();


            

        }
    }
}
