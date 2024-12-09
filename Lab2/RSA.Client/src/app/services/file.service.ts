import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface EncryptedFileResponse {
  encryptedContent: string;
  aesKey: string;
  iv: string;
}

@Injectable({
  providedIn: 'root',
})
export class FileService {
  private readonly apiUrl = 'http://localhost:5202';

  constructor(private readonly http: HttpClient) {}

  sendPublicKey(publicKey: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/key/send`, {
      key: publicKey,
    });
  }

  getServerPublicKey(): Observable<string> {
    return this.http.get(`${this.apiUrl}/key/get`, { responseType: 'text' });
  }

  requestFile(encryptedFileName: string): Observable<EncryptedFileResponse> {
    return this.http.post<EncryptedFileResponse>(
      `${this.apiUrl}/download`,
      { encryptedFileName },
      { responseType: 'json' }
    );
  }
}
