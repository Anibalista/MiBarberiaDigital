using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_SGBM.Helpers
{
    public class HelperError
    {
        public static void setErrorCampo(ErrorProvider provider, Control control,  string error = "")
        {
            provider.SetError(control, error);
        }

        
    }
}
