import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-produto-list',
  templateUrl: './produto-list.component.html'
})

export class ProdutoListComponent {
  public produtos: Produto[];

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router
  )
  {
    http.get<Produto[]>(baseUrl + 'api/produto/list').subscribe(result => {
      this.produtos = result;
    }, error => console.error(error));
  }

  excluir(prod: Produto)
  {
    this.http.delete<Number>(
      this.baseUrl + 'api/produto/' + prod.produtoID)
      .subscribe(result => {
        this.produtos = this.produtos.filter(x => x.produtoID !== prod.produtoID)
      }, error => console.error(error));;
  }

  async irParaFormProduto(id: string = 'new') {
    this.router.navigateByUrl(`produtos/${id}`);
  }
}

interface Produto {
  produtoID: number;
  nome: string;
  valorVenda: number;
  possuiImagem: boolean;
}
