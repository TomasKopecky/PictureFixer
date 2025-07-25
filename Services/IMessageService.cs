using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureFixer.Services
{
    public interface IMessageBoxService
    {
        void ShowError(string message);
    }
}
