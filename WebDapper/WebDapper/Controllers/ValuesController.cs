using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDbConnection _conn;

        public ValuesController(IDbConnection conn)
        {
            this._conn = conn;
        }
        [HttpGet]
        public IActionResult Index()
        {
            //Dapper查詢資料，注意不能用IEnumerable<DataRow>來接結果
            IEnumerable<dynamic> rows = this._conn.Query("Select * from tCustomer");

            StringBuilder sb = new StringBuilder();
            foreach (dynamic row in rows)
            {
                sb.AppendLine(row.tName);
            }//end foreach
            string result = sb.ToString();

            return Content(result);//直接顯示結果 
        }
        }
}
