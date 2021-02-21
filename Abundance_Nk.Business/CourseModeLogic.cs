using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CourseModeLogic : BusinessBaseLogic<CourseMode, COURSE_MODE>
    {
        public CourseModeLogic()
        {
            translator = new CourseModeTranslator();
        }
    }



}
