import { Component, OnInit } from '@angular/core';
import { AzureAIService } from '../Service/azure.service';


@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent implements OnInit {

  userid:string=''
  constructor(private azureservice:AzureAIService){}
  ngOnInit(): void {
    this.userid=localStorage.getItem('userid')?.toUpperCase()||"";
  }
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
  //const botReply = await this.azureservice.askAzureAI(lastUserMessage.text.toString());
  const botReply= (await this.azureservice.OpenAICall(this.userid,lastUserMessage.text.toString())).subscribe({
    next:result=>{
  this.messages.push({ sender: 'bot', text: result });
    },error:err=>{
      this.messages.push({ sender: 'bot', text: "error Please reach out technical Team." });
    }
  });

}
  //   this.askAzureAI(this.userInput).then(response => {
  // this.messages.push({ sender: 'bot', text: response });
//});
  }
}