using Microsoft.AspNetCore.Mvc;

// responsavel por criar o hosting -> aplicacao web.
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


//criando um endpoint que acessa o nome de um usuario;
//como retornar um json de uma api;
app.Map("/", () => new{Name = "Ana Vitoria", Age = 20});
app.MapGet("/", () => "Hello World!");

//alterar a resposta.
//passar para um metodo, **primeiro coloca o tipo**
//se eu quiser que a informação apareca no *body* é só abrir chave depois da setinha.
app.MapGet("/AddHeader", (HttpResponse response) => 
{
    response.Headers.Add("Teste", "Bianca");
    return  new{Name = "Ana Vitoria", Age = 20};
});

//criando endpoint, passa o metodo Product e chama o objeto product;
app.MapPost("/products", (Product product)=> {
    //retorna a concatenação de codigo do produto e nome desse determinado produto
    //enviar uma informação através do endpoint pelo bosy.
    //return product.Code + " - " + product.Name;
    ProductRepository.Add(product);
    return Results.Created("/products/{$product.Code}", product.Code); //retorno.
});

//passagem de parametros através da url; queryparametros.
//api.app.com/users?datastart={date}&dateend ={date}
//https://localhost:7001/getproduct?datestart=x&dateend=y
app.MapGet("/products", ([FromQuery] string dateStart, [FromQuery] string dateEnd) => {
    return dateStart + " - " + dateEnd;
});

//api.ap.com/user/{code} --> rota
//atraves do body.
//https://localhost:7001/getproduct/xpttO
app.MapGet("/products/{code}", ([FromRoute] string code) => {
    //return code;
    var product = ProductRepository.GetBy(code);
    //return product;
    if(product != null)
        return Results.Ok(product);
        return Results.NotFound();

});

//https://localhost:7001/getproductbyheader/
//parametro para troca de informacao com endpoint pelo header.
//app.MapGet("/getproductbyheader", (HttpRequest request)=> {
    //  return request.Headers["Product-Code"].ToString();
//});

//https://localhost:7001/editproduct
app.MapPut("/products", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code)=> {
  var productSaved = ProductRepository.GetBy(code);
  ProductRepository.Remove(productSaved);
  return Results.Ok();
});

app.Run();

public static class ProductRepository {
    public static List <Product> Products {get; set;}

    public static void Add(Product product){
        if(Products == null)
            Products = new List<Product> ();

            Products.Add(product);
    }
        public static Product GetBy(string code){
            return Products.FirstOrDefault(p => p.Code == code);
        }

        public static void Remove(Product product){
            Products.Remove(product);
        }
 }

public class Product {
    public string Code { get; set; }
    public string Name { get; set; }
}


