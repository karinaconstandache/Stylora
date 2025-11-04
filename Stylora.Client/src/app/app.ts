import { Component, signal } from '@angular/core';
import { VtonTryOnComponent } from './Components/vton-try-on/vton-try-on';

@Component({
  selector: 'app-root',
  imports: [VtonTryOnComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Stylora.Client');
}
