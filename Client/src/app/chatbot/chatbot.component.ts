import { Component } from '@angular/core';
import { AzureAIService } from '../Service/azure.service';


@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent {
  constructor(private azureservice:AzureAIService){}
  isOpen = false;
  messages: { sender: 'user' | 'bot', text: string }[] = [];
  userInput = '';

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  sendMessage() {
    if (!this.userInput.trim()) return;

    this.messages.push({ sender: 'user', text: this.userInput });
    // Placeholder bot response
    this.messages.push({ sender: 'bot', text: `You said: ${this.userInput}` });
    this.userInput = '';
    this.botmessage();
  }
  async botmessage(){
    
    const lastUserMessage = [...this.messages]
  .reverse()
  .find(msg => msg.sender === 'user');

console.log(lastUserMessage?.text);

    if (lastUserMessage?.text) {
  const botReply = await this.azureservice.askAzureAI(lastUserMessage.text.toString());
  this.messages.push({ sender: 'bot', text: botReply });
}
  //   this.askAzureAI(this.userInput).then(response => {
  // this.messages.push({ sender: 'bot', text: response });
//});
  }
}