using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Models.Result
{
    public abstract class DataTableTranslatorBase<T>
    {
        public static string ValidationMessage;

        protected int errorCount;
        protected List<StudentLevel> studentLevels;

        public DataTableTranslatorBase(List<StudentLevel> studentLevels)
        {
            this.studentLevels = studentLevels;
        }

        public abstract List<T> Translate(DataTable dataTable);
        protected abstract bool InvalidTypeOnSheet(DataTable excelData);
        protected abstract void Validate(T model);

        protected bool InvalidStudentLevelClassTerm(Func<StudentLevel, bool> selector)
        {
            try
            {
                StudentLevel studentLevel = studentLevels.Where(selector).SingleOrDefault();
                if (studentLevel == null)
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected bool DuplicateStudentIdExist(DataTable dataTable)
        {
            try
            {
                var duplicates = dataTable.AsEnumerable()
                 .GroupBy(r => r[0])
                 .Where(gr => gr.Count() > 1)
                 .Select(g => g.Key);

                if (duplicates != null && duplicates.Count() > 0)
                {
                    string idString = "";
                    var ids = duplicates.ToList();
                    for (int i = 0; i < ids.Count; i++)
                    {
                        idString += ids[i];
                        if (i < ids.Count - 1)
                        {
                            idString += ", ";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(idString))
                    {
                        SetErrorMessage("Duplicate Student ID of " + idString + " was detected! Student ID and Admission Number must be unique on the list. Please make necessary corrections, and re-upload.");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void SetErrorMessage(string errorMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ValidationMessage))
                {
                    ValidationMessage = errorMessage;
                }
                else
                {
                    ValidationMessage += ". --- " + errorMessage;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }





    }




}