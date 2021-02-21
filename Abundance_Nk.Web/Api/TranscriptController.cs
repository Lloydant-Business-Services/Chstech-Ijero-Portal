using Abundance_Nk.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Abundance_Nk.Web.Api
{
    public class TranscriptController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage ModifyTranscriptRequest(int transcriptRequestId, int StatusId)
        {
            TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
            var request=transcriptRequestLogic.GetModelsBy(f => f.Transcript_Request_Id == transcriptRequestId).FirstOrDefault();
            if (request?.Id > 0)
            {
                request.transcriptStatus = new Model.Model.TranscriptStatus { TranscriptStatusId = StatusId };
                var modified=transcriptRequestLogic.Modify(request);
                if (modified)
                    return Request.CreateResponse("success");

            }
            return Request.CreateResponse("Failed");
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}