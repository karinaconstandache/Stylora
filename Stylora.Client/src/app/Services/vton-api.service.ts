import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IVTONRequest } from '../Models/Interfaces/IVTONRequest';
import { IVTONResponse } from '../Models/Interfaces/IVTONResponse';

@Injectable({
  providedIn: 'root'
})
export class VTONApiService {
  // Use the URL of your C# Web API project
  private apiBaseUrl = 'http://localhost:5231/Vton'; 

  constructor(private http: HttpClient) { }

  tryOn(request: IVTONRequest): Observable<IVTONResponse> {
    return this.http.post<IVTONResponse>(`${this.apiBaseUrl}/try-on`, request);
  }
}