using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using sitemercado.web.Data;
using sitemercado.web.Models;
using System;
using System.Collections.Generic;
using io = System.IO ;
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
        readonly string absoluteProdutoUploadPath ;
                
        public ProdutoController(
            ApplicationDbContext dbContext,
            IWebHostEnvironment env
        )
        {
            _dbContext = dbContext;
            _env = env;
            absoluteProdutoUploadPath = _env.WebRootPath + "\\Content\\Produtos\\temp";
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
            _dbContext.SaveChanges();
        }


        [HttpGet("{id}")]
        public Produto Get(int id)
        {
            return _dbContext.Produtos.Single(x=>x.ProdutoID==id);
        }
       
        [HttpPost]
        public int Post(ProdutoInsertVM prod)
        {
            if (prod.prod.ProdutoID == 0) { 
                _dbContext.Produtos.Add(prod.prod);
            }
            else 
            {
                _dbContext.Update(prod.prod);
            }
            _dbContext.SaveChanges();
            
            if(prod.prod.PossuiImagem && prod.tempImage != null && Url.IsLocalUrl(prod.tempImage)) 
            { 
                var tempFile = _env.WebRootPath + prod.tempImage;
                //if (io.File.Exists(tempFile)) 
                //{
                    io.File.Move(tempFile, _env.WebRootPath + "\\content\\Produtos\\img_" + prod.prod.ProdutoID.ToString() + ".png", true);
                //}
                
            }

            return prod.prod.ProdutoID;
        }
        
        void AssertPathUploadProdutos() 
        {
            if (!io.Directory.Exists(absoluteProdutoUploadPath)) 
            { 
                io.Directory.CreateDirectory(absoluteProdutoUploadPath);
            }    
        }

        [HttpPost("upload")]
        //[RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        //[RequestSizeLimit(209715200)]
        public async Task<IActionResult> Upload()
        {
            const string relPath = "~/Content/Produtos/temp/";
            
            AssertPathUploadProdutos();

            //var formFile = formFiles.ElementAt(0);
            var formFile = Request.Form.Files.First();

            var fileName = string.Format("{0}{1}", Guid.NewGuid().ToString(), io.Path.GetExtension(formFile.FileName));

            var fullFilePath = io.Path.Combine(absoluteProdutoUploadPath, fileName);
            if (formFile.Length > 0)
            {
                using (var stream = System.IO.File.Create(fullFilePath))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
            

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { fileUrl = Url.Content(relPath + fileName)  });
        }

    }
    public class ProdutoInsertVM
    {
        public Produto prod { get; set; }
        public string tempImage { get; set; }
    }
}
