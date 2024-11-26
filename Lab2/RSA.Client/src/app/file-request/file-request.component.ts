import { Component, OnInit } from '@angular/core';
import { EncryptedFileResponse, FileService } from '../services/file.service';
import { ToastrService } from 'ngx-toastr';
import { mergeMap } from 'rxjs';
import { CryptoService } from '../services/crypto.service';

@Component({
  selector: 'app-file-request',
  templateUrl: './file-request.component.html',
  styleUrls: ['./file-request.component.css'],
})
export class FileRequestComponent implements OnInit {
  fileName: string = '';
  public serverPublicKey: string = '';

  constructor(
    private readonly fileService: FileService,
    private readonly cryptoService: CryptoService,
    private readonly toastr: ToastrService
  ) {}

  async ngOnInit() {
    await this.cryptoService.generateKeyPair();
    this.fetchServerPublicKey();
    await this.registerClientPublicKey();

    this.decryptFile = this.decryptFile.bind(this);
  }

  private fetchServerPublicKey() {
    this.fileService.getServerPublicKey().subscribe({
      next: (publicKey) => {
        this.serverPublicKey = publicKey;
        this.toastr.success('Публічний ключ сервера отримано.', 'Успіх');
      },
      error: (err) => {
        console.error('Помилка отримання ключа сервера:', err);
        this.toastr.error(
          'Не вдалося отримати публічний ключ сервера.',
          'Помилка'
        );
      },
    });
  }

  private async registerClientPublicKey() {
    const clientPublicKey = await this.cryptoService.exportRsaPublicKey();

    this.fileService.sendPublicKey(clientPublicKey).subscribe({
      next: () => {
        this.toastr.success(
          'Публічний ключ клієнта успішно зареєстровано.',
          'Успіх'
        );
      },
      error: (err) => {
        console.error('Помилка реєстрації ключа:', err);
        this.toastr.error(
          'Не вдалося зареєструвати публічний ключ.',
          'Помилка'
        );
      },
    });
  }

  async requestFile() {
    if (!this.fileName) {
      this.toastr.warning('Будь ласка, введіть назву файлу.', 'Попередження');
      return;
    }
    let encryptedFileName;

    try {
      encryptedFileName = await this.cryptoService.encryptWithRsa(
        this.fileName,
        this.serverPublicKey
      );
    } catch (err) {
      console.error('Помилка шифрування імені файлу:', err);
      this.toastr.error('Помилка шифрування імені файлу.', 'Помилка');
      return;
    }

    this.fileService
      .requestFile(encryptedFileName)
      .pipe(mergeMap(this.decryptFile))
      .subscribe({
        next: (response) => {
          if (!response) {
            this.toastr.error('Помилка розшифрування файлу.', 'Помилка');
          } else {
            this.downloadFile(response);
          }
        },
        error: (err) => {
          console.error('Помилка запиту файлу:', err);
          this.toastr.error('Не вдалося отримати файл.', 'Помилка');
        },
      });
  }

  private async decryptFile(response: EncryptedFileResponse) {
    const key = await this.cryptoService.decryptWithRsaPrivateKey(
      response.aesKey
    );
    const iv = await this.cryptoService.decryptWithRsaPrivateKey(response.iv);

    if (!key || !iv) {
      return null;
    }
    await this.cryptoService.importAesKey(key);

    const decryptedContent = await this.cryptoService.decryptWithAes(
      iv,
      response.encryptedContent
    );

    return decryptedContent;
  }

  private downloadFile(fileData: ArrayBuffer) {
    const blob = new Blob([fileData], {
      type: 'application/octet-stream',
    });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = this.fileName;
    a.click();
    window.URL.revokeObjectURL(url);
    this.toastr.success('Файл успішно розшифровано та завантажено.', 'Успіх');
  }
}
