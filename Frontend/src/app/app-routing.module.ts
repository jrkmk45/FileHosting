import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { PublicFilesPageComponent } from './pages/public-files-page/public-files-page.component';
import { FilePageComponent } from './pages/file-page/file-page.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  {path:'', component: MainPageComponent, canActivate: [AuthGuard]},
  {path:'register', component:RegisterPageComponent},
  {path:'login', component:LoginPageComponent},
  {path:'files/public', component: PublicFilesPageComponent},
  {path:'files/:id', component: FilePageComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true,  scrollPositionRestoration: 'top', anchorScrolling: 'enabled'})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
