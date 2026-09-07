import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { OperacionesComponent } from './app/operaciones.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [OperacionesComponent],
  template: `<app-operaciones />`
})
export class AppComponent {}

bootstrapApplication(AppComponent, {
  providers: [provideHttpClient()]
}).catch((err) => console.error(err));
