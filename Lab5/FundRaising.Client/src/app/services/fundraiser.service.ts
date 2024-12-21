import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FundraiserDto } from '../models/Fundraiser/FundraiserDto';
import { CreateFundraiserDto } from '../models/Fundraiser/CreateFundraiserDto';
import { UpdateFundraiserDto } from '../models/Fundraiser/UpdateFundraiserDto';
import { PaymentDto } from '../models/Fundraiser/PaymentDto';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class FundraiserService {
  private apiUrl = 'http://localhost:5200/fundraisers';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getHeaders(): HttpHeaders {
    return this.authService.getAuthHeaders();
  }

  getAll(): Observable<FundraiserDto[]> {
    const headers = this.getHeaders();
    return this.http.get<FundraiserDto[]>(this.apiUrl, { headers });
  }

  getById(id: string): Observable<FundraiserDto> {
    const headers = this.getHeaders();
    return this.http.get<FundraiserDto>(`${this.apiUrl}/${id}`, { headers });
  }

  create(fundraiser: CreateFundraiserDto): Observable<FundraiserDto> {
    const headers = this.getHeaders();
    return this.http.post<FundraiserDto>(this.apiUrl, fundraiser, { headers });
  }

  update(id: string, fundraiser: UpdateFundraiserDto): Observable<void> {
    const headers = this.getHeaders();
    return this.http.put<void>(`${this.apiUrl}/${id}`, fundraiser, { headers });
  }

  delete(id: string): Observable<void> {
    const headers = this.getHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

  donate(fundraiserId: string, payment: PaymentDto): Observable<FundraiserDto> {
    return this.http.post<FundraiserDto>(`/api/fundraisers/${fundraiserId}/donate`, payment);
  }  
}
