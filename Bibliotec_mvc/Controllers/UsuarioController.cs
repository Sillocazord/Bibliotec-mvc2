using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bibliotec_mvc.Controllers
{
    [Route("[controller]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
        }

        //Criando um obj da classe context
        Context context = new Context();

        //O metódo index esta retornando a View Usuario/Index.cshtml (se tiver com letra pequena nao funciona)
        public IActionResult Index()
        {
            //Pegar as informações da session que são necessárias para que aparece os detalhes do meu usuário
            int id = int.Parse(HttpContext.Session.GetString("UsuarioID")!);
            ViewBag.Admin = HttpContext.Session.GetString("Admin")!;

            // id = 1 (Vai encontrar o usuario e suas informações(nome, email, contato, etc...))
            //buscar usuario que esta logado
            Usuario usuarioEncontrado = context.Usuario.FirstOrDefault(usuario => usuario.UsuarioID == id)!;
            //se nao for encontrado ninguém...
            if (usuarioEncontrado == null)
            {
                return NotFound();
            }

            // procurar o curso que o meu usuario esta cadastrado
            Curso cursoEncontrado = context.Curso.FirstOrDefault(curso => curso.CursoID == usuarioEncontrado.CursoID)!;

            //Verificar se o usuário possui ou não o curso
            if (cursoEncontrado == null)
            {
                //"O usuario não possui curso cadastrado"
                //preciso que vc mande essa mensagem para a view
                ViewBag.Curso = "O usuário não possui curso cadastrado";
            }
            else
            {
                //"O usuário possui o curso XXX"
                //Preciso que mande o nome do curso para view
                ViewBag.Curso = cursoEncontrado.Nome;
            }

            ViewBag.Nome = usuarioEncontrado.Nome;
            ViewBag.Email = usuarioEncontrado.Email;
            ViewBag.Zap = usuarioEncontrado.Contato;
            ViewBag.DtNasc = usuarioEncontrado.DtNascimento.ToString("dd/MM/yyyy");

            //resumindo, esse negocio pega as tabelas criadas la no sql server, e seleciona as informacoes na tabela e exibe aqui, estamos pegando as tabelas e jogando aqui no vs code, oque vai jogar elas no site e fazer com que o usuario veja as informações, pra ele vai estar prontinho, pra nois vai ter sido o negocio mais trabalhoso das nossas vidas kkkkkkkkkk

            //tabela usuario -> FK CursoID
            //Tabela Curso -> PK CursoID
            //Dev integral -> CursoID = 6
            //Hiorhanna -> CursoID = 6
            return View();
        }


        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}