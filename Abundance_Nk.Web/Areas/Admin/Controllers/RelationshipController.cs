
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Business;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class RelationshipController : BasicSetupBaseController<Relationship, RELATIONSHIP>
    {
        public RelationshipController() : base(new RelationshipLogic())
        {
            ModelName = "Relationship";
            Selector = r => r.Relationship_Id == Id;
        }

        protected override bool ModifyModel(Relationship model)
        {
            try
            {
                RelationshipLogic relationshipLogic = new RelationshipLogic();
                return relationshipLogic.Modify(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

     



	}
}