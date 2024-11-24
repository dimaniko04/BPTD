import { Component, OnInit } from '@angular/core';
import { JSEncrypt } from 'jsencrypt';
import { FileService } from '../services/file.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-file-request',
  templateUrl: './file-request.component.html',
  styleUrls: ['./file-request.component.css'],
})
export class FileRequestComponent implements OnInit {
  fileName: string = '';
  private clientPublicKey: string = '';
  private clientPrivateKey: string = '';
  public serverPublicKey: string = '';

  constructor(private fileService: FileService, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.generateClientKeys();
    this.fetchServerPublicKey();
    this.registerClientPublicKey();
  }

  private generateClientKeys() {
    const rsa = new JSEncrypt({ default_key_size: '2048' });
    this.clientPrivateKey = rsa.getPrivateKey() as string;
    this.clientPublicKey = rsa.getPublicKey() as string;
  }

  private fetchServerPublicKey() {
    this.fileService.getServerPublicKey().subscribe({
      next: (publicKey) => {
        this.serverPublicKey = publicKey;
        this.toastr.success('Публічний ключ сервера отримано.', 'Успіх');
      },
      error: (err) => {
        console.error('Помилка отримання ключа сервера:', err);
        this.toastr.error('Не вдалося отримати публічний ключ сервера.', 'Помилка');
      },
    });
  }

  private registerClientPublicKey() {
    this.fileService.sendPublicKey(this.clientPublicKey).subscribe({
      next: () => {
        this.toastr.success('Публічний ключ клієнта успішно зареєстровано.', 'Успіх');
      },
      error: (err) => {
        console.error('Помилка реєстрації ключа:', err);
        this.toastr.error('Не вдалося зареєструвати публічний ключ.', 'Помилка');
      },
    });
  }

  requestFile() {
    if (!this.fileName) {
      this.toastr.warning('Будь ласка, введіть назву файлу.', 'Попередження');
      return;
    }
  
    const encryptor = new JSEncrypt();
    encryptor.setPublicKey(this.serverPublicKey);
    const encryptedFileName = encryptor.encrypt(this.fileName);
  
    if (!encryptedFileName) {
      this.toastr.error('Помилка шифрування імені файлу.', 'Помилка');
      return;
    }
  
    this.fileService.requestFile(encryptedFileName).subscribe({
      next: (encryptedResponse) => {
        console.log('encryptedResponse', encryptedResponse);
        this.decryptAndDownloadFile(encryptedResponse);
      },
      error: (err) => {
        console.error('Помилка запиту файлу:', err);
        this.toastr.error('Не вдалося отримати файл.', 'Помилка');
      },
    });
  }  
  
  private decryptAndDownloadFile(encryptedData: string) {
    // const decryptor = new JSEncrypt();
    // decryptor.setPrivateKey(this.clientPrivateKey);
    // const decryptedData = decryptor.decrypt(encryptedData);
  
    // if (!decryptedData) {
    //   this.toastr.error('Помилка розшифрування файлу.', 'Помилка');
    //   return;
    // }
  
    // const blob = new Blob([decryptedData], { type: 'application/octet-stream' });
    // const url = window.URL.createObjectURL(blob);
    // const a = document.createElement('a');
    // a.href = url;
    // a.download = this.fileName;
    // a.click();
    // window.URL.revokeObjectURL(url);
  
    // this.toastr.success('Файл успішно розшифровано та завантажено.', 'Успіх');
  } 
}
