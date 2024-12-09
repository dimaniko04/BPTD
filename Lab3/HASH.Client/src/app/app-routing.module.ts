import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HashUiComponent } from './components/hash-ui/hash-ui.component';

const routes: Routes = [
  { path: 'hash', component: HashUiComponent },
  { path: '', redirectTo: '/hash', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
