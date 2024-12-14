import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FundraiserDto } from '../models/Fundraiser/FundraiserDto';
import { CreateFundraiserDto } from '../models/Fundraiser/CreateFundraiserDto';

@Injectable({ providedIn: 'root' })
export class FundraiserService {
  private apiUrl = 'http://localhost:5000/fundraisers';

  constructor(private http: HttpClient) {}

  getAll(): Observable<FundraiserDto[]> {
    return this.http.get<FundraiserDto[]>(this.apiUrl);
  }

  getById(id: string): Observable<FundraiserDto> {
    return this.http.get<FundraiserDto>(`${this.apiUrl}/${id}`);
  }

  create(fundraiser: CreateFundraiserDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, fundraiser);
  }

  update(id: string, fundraiser: CreateFundraiserDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, fundraiser);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}