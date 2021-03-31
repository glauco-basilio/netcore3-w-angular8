import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'produto-form-component',
  templateUrl: './produto-form.component.html'
})
export class ProdutoFormComponent implements OnInit {
  id: string;
  imagemUrl: string = 'http://dracybeledepaiva.com.br/wp-content/uploads/2016/10/orionthemes-placeholder-image.png';
  model: Produto = {}
  exibeErro: boolean = false
  exibeSucesso: boolean = false
  nomeInvalido: boolean = false
  valorVendaInvalido: boolean = false

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    @Inject('BASE_URL')
    private baseUrl: string
  ) {
    const id:string = route.snapshot.paramMap.get("id");
    if (id !== 'new')
    {
      this.getProduto(id)
    }
  }

  getProduto(id: string)
  {
    this.http.get<Produto>(this.baseUrl + 'api/produto/' + id )
      .subscribe(result => {
        this.model = result;
      }, error => console.error(error));
  }

  oFormEValido(): boolean
  {
    this.nomeInvalido = false
    this.valorVendaInvalido = false
    if (!this.model.nome)
    {
      this.nomeInvalido = true
    }
    if ((!this.model.valorVenda) || this.model.valorVenda <= 0)
    {
      this.valorVendaInvalido = true
    }
    return !(this.valorVendaInvalido || this.nomeInvalido)
  }

  salvar() {
    if (this.oFormEValido()) {
      this.http.post<Produto>(this.baseUrl + 'api/produto', this.model)
        .subscribe(result => {
          this.model.produtoID = Number(result);
          this.salvoComSucesso();
        }, error => {
          console.error(error)
          this.erroAoSalvar();
        });
    }
  }

  salvoComSucesso()
  {
    this.exibeErro = !( this.exibeSucesso = true )
  }

  erroAoSalvar()
  {
    this.exibeErro = !(this.exibeSucesso = false)
  }

  async ngOnInit() {
    if (this.model.possuiImagem) {
      this.imagemUrl = 'img_' + this.model + '.png';
    }
  }
}

interface Produto {
  produtoID?: number;
  nome?: string;
  valorVenda?: number;
  possuiImagem?: boolean;
}
