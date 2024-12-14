import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FundraisersComponent } from './components/fundraisers/fundraisers.component';

const routes: Routes = [
  { path: '', redirectTo: '/fundraisers', pathMatch: 'full' },
 // { path: 'login', component: LoginComponent },
 // { path: 'register', component: RegisterComponent },
  { path: 'fundraisers', component: FundraisersComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
