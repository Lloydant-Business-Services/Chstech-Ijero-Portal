using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Translator;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Business
{
    public class TranscriptStatusLogic : BusinessBaseLogic<TranscriptStatus, TRANSCRIPT_STATUS>
    {
        public TranscriptStatusLogic()
        {
            translator = new TranscriptStatusTranslator() ;
        }
    }
}
