import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';

@Component({
  selector: 'produto-form-component',
  templateUrl: './produto-form.component.html'
})
export class ProdutoFormComponent  {
  id: string;
  imagemUrl: string = 'http://dracybeledepaiva.com.br/wp-content/uploads/2016/10/orionthemes-placeholder-image.png';
  model: Produto = {}
  exibeErro: boolean = false
  exibeSucesso: boolean = false
  nomeInvalido: boolean = false
  valorVendaInvalido: boolean = false
  progress: number;
  uploading: boolean;
  fileUrl: string;

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
        this.assertImagem();
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

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', this.baseUrl + 'api/produto/upload', formData, {
      reportProgress: true
    });
    this.uploading = true;
    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.uploading = false;
        this.model.tempImage = 
          this.imagemUrl = (event.body as any).fileUrl;
        this.model.possuiImagem = true;
      }
    });
  }

  salvar() {
    if (this.oFormEValido()) {
      this.http.post<ProdutoView>(
        this.baseUrl + 'api/produto',
         { prod: this.model, tempImage: this.imagemUrl }
      )
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

  assertImagem() {
    if (this.model.possuiImagem) {
      this.imagemUrl = this.baseUrl + '/content/Produtos/img_' + this.model.produtoID + '.png?' + (new Date()).getTime();
    }
  }
}

interface Produto {
  produtoID?: number;
  nome?: string;
  valorVenda?: number;
  possuiImagem?: boolean;
  tempImage?: string;
}

interface ProdutoView {
  prod: Produto;
  tempImage: string;
}
