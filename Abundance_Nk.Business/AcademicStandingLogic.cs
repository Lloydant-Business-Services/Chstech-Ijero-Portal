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
    public class AcademicStandingLogic : BusinessBaseLogic<AcademicStanding, ACADEMIC_STANDING>
    {
        public AcademicStandingLogic()
        {
            translator = new AcademicStandingTranslator();
        }

        public string GetAbbreviations()
        {
            try
            {
                string abbreviations = null;
                List<AcademicStanding> academicStandings = GetAll().OrderBy(g => g.Id).ToList();
                if (academicStandings != null && academicStandings.Count > 0)
                {
                    abbreviations += "ABBREVIATIONS:- ";
                    for (int i = 0; i < academicStandings.Count; i++ )
                    {
                        abbreviations += academicStandings[i].Abbreviation + "-" + academicStandings[i].Name;
                        if (i != academicStandings.Count - 1)
                        {
                            abbreviations += "; ";
                        }
                    }                    
                }

                return abbreviations;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }





}
