import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FileRequestComponent } from './file-request/file-request.component';

const routes: Routes = [
  { path: 'request-file', component: FileRequestComponent },
  { path: '', redirectTo: '/request-file', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
