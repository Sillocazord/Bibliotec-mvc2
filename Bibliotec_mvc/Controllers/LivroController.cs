using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bibliotec_mvc.Controllers
{
    [Route("[controller]")]
    public class LivroController : Controller
    {
        private readonly ILogger<LivroController> _logger;

        public LivroController(ILogger<LivroController> logger)
        {
            _logger = logger;
        }

        Context context = new Context();

        public IActionResult Index()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin")!;

            List<Livro> listaLivros = context.Livro.ToList();
            //Verificar se o livro tem reserva ou não
            var livrosReservados = context.LivroReserva.ToDictionary(livro => livro.LivroID, livror => livror.DtReserva);

            ViewBag.Livros = listaLivros;
            ViewBag.livrosComReserva = livrosReservados;


            return View();
        }

        [Route("Cadastro")]
        //Metodo que retorna a tela de cadastro
        public IActionResult Cadastro()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin")!;

            ViewBag.Categorias = context.Categoria.ToList();
            //Retorna a view de cadastro meus crias(Whatsuuuuuuuup!)🔥🔥🔥💥💥💥📢📢📢🗿🗿🗿👊👊👊⚡⚡⚡🏹🏹🏹🗡️🗡️🗡️😁😁😁
            return View();
        }

        // Método para cadastrar um Livro:

        [Route("Cadastrar")]
        public IActionResult Cadastrar(IFormCollection form)
        {

            //1- PARTE: CADASTRAR UM LIVRO NA TABELA LIVRO

            Livro novoLivro = new Livro();

            //Oque meu usuario escrever no formulario, sera atribuido ao livro novo (novoLivro)
            novoLivro.Nome = form["Nome"].ToString();
            novoLivro.Editora = form["Editora"].ToString();
            novoLivro.Escritor = form["Escritor"].ToString();
            novoLivro.Idioma = form["Idioma"].ToString();
            novoLivro.Descricao = form["Descricao"].ToString();
            //Parte de colocar a imagem meus crias(socorro que isso vai dar trabalho💀☠️)
            if (form.Files.Count > 0)
            {
                //Primeiro Passo = Amazenar o arquivo enviado pelo usuário
                var arquivo = form.Files[0];

                //Segundo Passo = Criar variavel do caminnho da minha pasta para colocar as fotos dos livros

                // Validaremos se a pasta que será armazenada as imagens, exista. Caso não exista, criaremos uma nova pasta (chuta que é macumba)
                var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Livros");

                if (!Directory.Exists(pasta))
                {
                    //Criar a pasta caso ela não exista(me chama de Lord rithg?)
                    Directory.CreateDirectory(pasta);
                }
                //Terceiro passo:
                var caminho = Path.Combine(pasta, arquivo.FileName);

                using (var stream = new FileStream(caminho, FileMode.Create)){
                 //  Copiou o arquivo para o seu diretorio  
                 arquivo.CopyTo(stream);

                }

                novoLivro.Imagem = arquivo.FileName;
               
            } else{
                novoLivro.Imagem = "padrao.png";
                
            }

            //img
            context.Livro.Add(novoLivro);
            context.SaveChanges();
            //2- PARTE: ADICIONAR DENTRO DE LIVRO CATEGORIA, A CATEGORIA QUE PERTENCE AO novoLivro
            List<LivroCategoria> listalivroCategorias = new List<LivroCategoria>(); //Lista as categorias
            //Array que possui as categorias selecionadas pelo usuario

            string[] categoriasSelecionadas = form["Categoria"].ToString().Split(',');
            //acao,terror,suspense.
            //3,5,7

            foreach (string categoria in categoriasSelecionadas)
            {
                //string categoria possui a informação do id da categoria ATUAL selecionada
                LivroCategoria livroCategoria = new LivroCategoria();

                livroCategoria.CategoriaID = int.Parse(categoria);
                livroCategoria.LivroID = novoLivro.LivroID;
                //Adicionamos o obj livroCategoria dentro da lista listaLivroCategoria
                listalivroCategorias.Add(livroCategoria);
            }
            context.LivroCategoria.AddRange(listalivroCategorias);
            context.SaveChanges();

            return LocalRedirect("/Livro/Cadastro");
        }
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!"); OTC REFERENCE??, ACKNOLEGDE OR TRIBAL THIEF!!☝️☝️☝️
        // }
    }
}