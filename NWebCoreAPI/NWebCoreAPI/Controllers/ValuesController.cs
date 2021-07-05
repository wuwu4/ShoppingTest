using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NWebCoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NWebCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly TodoContext db;
        public ValuesController(TodoContext _db)
        {
            db = _db;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<TCustomer> Get() 
        {
            return db.TCustomers.ToList();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{fCustId}")]
        public ActionResult<TCustomer> Get(string fCustId)
        {
            var result = db.TCustomers.Find(fCustId);
            if (result == null)
            {
                return NotFound("找不到資源");
            }
            return result;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public ActionResult<TCustomer> Post([FromBody] TCustomer value)
        {
            db.TCustomers.Add(value);
            db.SaveChanges();
            return CreatedAtAction(nameof(Get), value);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{fCustId}")]
        public IActionResult Put(string fCustId, [FromBody] TCustomer value)
        {
            if (fCustId == null)
            {
                return BadRequest();
            }
            db.Entry(value).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (!db.TCustomers.Any(e => e.FCustId == fCustId))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "存取發生錯誤");
                }                
            }
                return NoContent();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{fCustId}")]
        public IActionResult Delete(string fCustId)
        {
            var delete = db.TCustomers.Find(fCustId);
            if (delete == null)
            {
                return NotFound();
            }
            db.TCustomers.Remove(delete);
            db.SaveChanges();
            return NoContent();
        }
    }
}
