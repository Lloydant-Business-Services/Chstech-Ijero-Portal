using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Business
{
    public class UtmeScreeningLogic
    {

        public decimal ScreenApplicant(List<OLevelResultDetail> oLevelResultDetails, ApplicantJambDetail applicantJambDetail, int Sitting)
        {
            decimal ScreeningScore = 0;
            try
            {
               
               
                 ScreeningScore += GetOlevelGradeScore(oLevelResultDetails,Sitting);
                 ScreeningScore += GetJambScoreFromRange(Convert.ToInt32(applicantJambDetail.JambScore));
            }
            catch (Exception ex)
            {
                    
                throw ex;
            }
            return ScreeningScore;
        }

        public int GetOlevelGradeScore(List<OLevelResultDetail> oLevelResultDetails, int Sitting)
        {
            int score = 0;
            try
            {
                if (Sitting == 1)
                {
                    score += 10;
                }
                else if (Sitting == 2)
                {
                    score += 6;
                }
                if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                {
                    foreach (OLevelResultDetail oLevelResultDetail in oLevelResultDetails)
                    {
                        score += GetGradeValue(oLevelResultDetail.Grade.Name.ToUpper());
                    }
                }

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return score;
        }

        public int GetGradeValue(string grade)
        {
            int score = 0;
            try
            {
                if (!string.IsNullOrEmpty(grade))
                {
                    switch (grade.ToUpper().Trim())
                    {
                        case "A1":
                            score = 6;
                            break;
                        case "B2":
                            score = 5;
                            break;
                        case "B3":
                            score = 4;
                            break;
                        case "C4":
                            score = 3;
                            break;
                        case "C5":
                            score = 2;
                            break;
                        case "C6":
                            score = 1;
                            break;
                    }
                }
                
                return score;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public decimal GetJambScoreFromRange(int jambScore)
        {
            decimal score = 0;
            try
            {
                score = Convert.ToDecimal(jambScore * 0.15M);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return score;
        }

    }
}
