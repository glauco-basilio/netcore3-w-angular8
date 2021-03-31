using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using sitemercado.web.Data;
using sitemercado.web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace sitemercado.web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
       

        private readonly ApplicationDbContext _dbContext;
        IWebHostEnvironment _env;
        public ProdutoController(
            ApplicationDbContext dbContext,
            IWebHostEnvironment env
        )
        {
            _dbContext = dbContext;
            _env = env;
        }

        [HttpGet("list")]
        public IEnumerable<Produto> List()
        { 
            var retorno = _dbContext.Produtos.ToList();

            return retorno;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var retorno = 
                _dbContext.Produtos.Remove(
                    _dbContext.Produtos.Single(x=>x.ProdutoID == id)
                );
        }


        [HttpGet("{id}")]
        public Produto Get(int id)
        {
            return _dbContext.Produtos.Single(x=>x.ProdutoID==id);
        }

        [HttpPost]
        public int Post(Produto p)
        {
            if (p.ProdutoID == 0) { 
                _dbContext.Produtos.Add(p);
            }
            else 
            {
                _dbContext.Update(p);
            }
            _dbContext.SaveChanges();
            return p.ProdutoID;
        }

        [HttpPost("UploadAsync")]
        public async Task<IActionResult> UploadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }

    }
}
