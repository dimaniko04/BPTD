import { Component } from '@angular/core';
import { HashService } from '../../services/hash.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-hash-ui',
  templateUrl: './hash-ui.component.html',
  styleUrls: ['./hash-ui.component.css'],
})
export class HashUiComponent {
  textInput = '';
  bitSize = 2;
  textHashResult = '';
  fileHashResult = '';
  textDigest = '';
  textVerificationResult = '';
  fileDigest = '';
  fileVerificationResult = '';
  selectedFile: File | null = null;
  collisionFile: File | null = null;

  constructor(
    private hashService: HashService,
    private toastr: ToastrService
  ) {}

  hashText() {
    if (!this.textInput.trim()) {
      this.toastr.error('Please enter text to hash.', 'Error');
      return;
    }
    this.hashService.hashText(this.textInput, this.bitSize).subscribe(
      (hash) => {
        this.textHashResult = hash;
        this.toastr.success('Text hashed successfully!', 'Success');
      },
      (error) => {
        console.error(error);
        this.toastr.error('Failed to hash text.', 'Error');
      }
    );
  }

  triggerFileInputClick() {
    const fileInput = document.querySelector('.file-input') as HTMLElement;
    fileInput?.click();
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  onFileDropped(event: DragEvent) {
    event.preventDefault();
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.selectedFile = files[0];
      this.toastr.info(`File selected: ${this.selectedFile.name}`, 'File Selection');
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.toastr.info(`File selected: ${this.selectedFile.name}`, 'File Selection');
    }
  }

  hashFile() {
    if (!this.selectedFile) {
      this.toastr.error('Please select a file to hash.', 'Error');
      return;
    }
    this.hashService.hashFile(this.selectedFile, this.bitSize).subscribe(
      (hash) => {
        this.fileHashResult = hash;
        this.toastr.success('File hashed successfully!', 'Success');
      },
      (error) => {
        console.error(error);
        this.toastr.error('Failed to hash file.', 'Error');
      }
    );
  }

  verifyText() {
    if (!this.textInput.trim()) {
      this.toastr.error('Please enter text to verify.', 'Error');
      return;
    }
    if (!this.textDigest.trim()) {
      this.toastr.error('Please enter a digest to verify.', 'Error');
      return;
    }
    this.hashService.verifyText(this.textInput, this.textDigest, this.bitSize).subscribe(
      (isValid) => {
        this.textVerificationResult = isValid ? 'Valid' : 'Invalid';
        const message = isValid ? 'Text verification successful!' : 'Text verification failed!';
        this.toastr.success(message, 'Verification');
      },
      (error) => {
        console.error(error);
        this.toastr.error('Failed to verify text.', 'Error');
      }
    );
  }

  verifyFile() {
    if (!this.selectedFile) {
      this.toastr.error('Please select a file to verify.', 'Error');
      return;
    }
    if (!this.fileDigest.trim()) {
      this.toastr.error('Please enter a digest to verify.', 'Error');
      return;
    }
    this.hashService.verifyFile(this.selectedFile, this.fileDigest, this.bitSize).subscribe(
      (isValid) => {
        this.fileVerificationResult = isValid ? 'Valid' : 'Invalid';
        const message = isValid ? 'File verification successful!' : 'File verification failed!';
        this.toastr.success(message, 'Verification');
      },
      (error) => {
        console.error(error);
        this.toastr.error('Failed to verify file.', 'Error');
      }
    );
  }

  generateCollision() {
    if (!this.selectedFile) {
      this.toastr.error('Please select a file to generate a collision.', 'Error');
      return;
    }
    this.hashService.generateFileCollision(this.selectedFile, this.bitSize).subscribe(
      (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'collision_' + this.selectedFile!.name;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Collision file generated and downloaded!', 'Success');
      },
      (error) => {
        console.error(error);
        this.toastr.error('Failed to generate collision file.', 'Error');
      }
    );
  }
}
