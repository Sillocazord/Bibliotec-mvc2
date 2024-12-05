using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

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

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    //  Copiou o arquivo para o seu diretorio  
                    arquivo.CopyTo(stream);

                }

                novoLivro.Imagem = arquivo.FileName;

            }
            else
            {
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

                livroCategoria.CategoriaID = int.Parse(categoria); //ERRO AQUI DUDE
                livroCategoria.LivroID = novoLivro.LivroID;
                //Adicionamos o obj livroCategoria dentro da lista listaLivroCategoria
                listalivroCategorias.Add(livroCategoria);
            }
            context.LivroCategoria.AddRange(listalivroCategorias);
            context.SaveChanges();

            return LocalRedirect("/Livro/Cadastro");
        }

        [Route("Editar/{id}")] //vai permitira abrir a pagina editar e chamar um livro especifico pelo Id para edita-ló
        public IActionResult Editar(int id)
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin")!;
            ViewBag.CategoriasDoSistema = context.Categoria.ToList();

            Livro livroEncontrado = context.Livro.FirstOrDefault(livro => livro.LivroID == id)!;
            //Now we gonna serach for categories that livroEncotrado have
            var categoriasDoLivroEncontrado = context.LivroCategoria.Where(identificadorLivro => identificadorLivro.LivroID == id).Select(livro => livro.Categoria).ToList();
            //Quero pegar as informações e mandar pra view, pra isso usa o view bag (Becausa im bad, im bad, jamone, HHHHHUUUUULL (yes, isso é uma musica do michael))
            ViewBag.livro = livroEncontrado;
            ViewBag.Categoria = categoriasDoLivroEncontrado;

            //diario de um banana. LivroId = 3 (ou banana kkkkk)
            //Buscar o crazy do id number 3 (omg michael)
            return View();
        }

        //metodo que atualiza as informações do livro
        [Route("Atualizar/{id}")]
        public IActionResult Atualizar(IFormCollection form, int id, IFormFile imagem)
        {
            //Buscar um livro especifico pelo ID
            Livro livroAtualizado = context.Livro.FirstOrDefault(livro => livro.LivroID == id)!;

            livroAtualizado.Nome = form["Nome"];
            livroAtualizado.Escritor = form["Escritor"];
            livroAtualizado.Editora = form["Editora"];
            livroAtualizado.Idioma = form["Idioma"];
            livroAtualizado.Descricao = form["Descricao"];
            //upload da imagem
            if (imagem!= null && imagem.Length > 0)
            {
                //(eu quero dormir😭😭😭)definir o caminho da minha imagem
                var caminhoImagem = Path.Combine("wwwroot/imagens/Livros", imagem.FileName);

                //verificar se o usuario colocou uma imagem para atualizar o livro
                //caso exista, ela irá ser excluida
                if (!string.IsNullOrEmpty(livroAtualizado.Imagem))
                {
                    var caminhoImagemAntiga = Path.Combine("wwwroot/imagens/Livros", livroAtualizado.Imagem);

                    //ver se existe uma imagem no caminho antigo
                    if (System.IO.File.Exists(caminhoImagemAntiga))
                    {
                        System.IO.File.Delete(caminhoImagemAntiga);
                    }//terceiro if

                }//segundo if

                //salvar a imagem nova
                using(var stream = new FileStream(caminhoImagem, FileMode.Create))
                {
                    imagem.CopyTo(stream); //Se falar copytu muito rapido fica parecido como capitu kkkkkkkkkkkkkkkkk
                }

                //Subir essa mudanca pro meu banco de dados
                livroAtualizado.Imagem = imagem.FileName;

            }// if primerio
             //Categorias:                (NAAAO, MAIS COISA PRA FAZER NAAO!!😭😭😭😭)
             //Primeiro: precisamos pegar as categorias selecionadas do usuario
            var categoriasSelecionadas = form["Categoria"].ToList();
            //Segundo: Pegaremos as cateforias Atuais do livro
            var categoriasAtuais = context.LivroCategoria.Where(livro => livro.LivroID == id).ToList();
            //Terceiro: Removeremos as categorias antigas
            foreach (var categoria in categoriasAtuais)
            {
                if (!categoriasSelecionadas.Contains(categoria.CategoriaID.ToString()))
                {
                    //nos vamos remover as categorias do nosso context
                    context.LivroCategoria.Remove(categoria);
                }
            }
            //Quarto: Adicionaremos as novas categorias
            foreach (var categoria in categoriasSelecionadas)
            {
                //verificando se não existe a categoria nesse livro
                if (!categoriasAtuais.Any(c => c.CategoriaID.ToString() == categoria))
                {
                    context.LivroCategoria.Add(new LivroCategoria
                    {
                        LivroID = id,
                        CategoriaID = int.Parse(categoria)
                    });
                }
            }

            context.SaveChanges();

            return LocalRedirect("/Livro");
        }

        [Route("Excluir")]
        public IActionResult Excluir(int id){
            Livro livroEncontrado = context.Livro.First(livro => livro.LivroID == id);

            //Buscar as categorias desse livro
            var categoriasDoLivro = context.LivroCategoria.Where(livro => livro.LivroID == id).ToList();

            //Precisa excluir primeiro o registro da tabela intermediaria
            foreach(var categoria){
                context.LivroCategoria.Remove(categoria);
            }

            context.Livro.Remove(livroEncontrado);

            context.SaveChanges();


            return LocalRedirect("/Livro");
        }
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!"); OTC REFERENCE??, ACKNOLEGDE OR TRIBAL THIEF!!☝️☝️☝️
        // }
    }
}