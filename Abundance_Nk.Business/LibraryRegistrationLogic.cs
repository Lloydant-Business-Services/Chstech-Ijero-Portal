using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class LibraryRegistrationLogic: BusinessBaseLogic<LibraryRegistration, LIBRARY_REGISTRATION>
    {
        public LibraryRegistrationLogic()
        {
            translator = new LibraryRegistrationTranslator();
        }
    }
}
