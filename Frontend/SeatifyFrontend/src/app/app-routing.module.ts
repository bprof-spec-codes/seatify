import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { CreateEventComponent } from './pages/create-event/create-event.component';
import { UpdateEventComponent } from './pages/create-event/update-event.component';

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  {path: 'landingpage', component: LandingPageComponent},
  {path: 'create-event', component:CreateEventComponent},
  {path: 'update-event', component:UpdateEventComponent},
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
