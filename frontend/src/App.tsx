import React, { useState, useEffect } from 'react'; 
import { authApi, messagesApi, inboxApi } from './api';
import type { InboxMessage } from './types';

export default function App() {
  const [token, setToken] = useState<string | null>(null);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoginView, setIsLoginView] = useState(true);
  
  const [recipient, setRecipient] = useState('');
  const [subject, setSubject] = useState('');
  const [body, setBody] = useState('');
  const [messages, setMessages] = useState<InboxMessage[]>([]);

  const fetchInbox = async () => {
    if (!token) return;
    try {
      const data = await inboxApi.list(token);
      setMessages(data || []);
    } catch (err) {
      console.error("Mesajlar alınamadı:", err);
    }
  };

  useEffect(() => {
    if (token) fetchInbox();
  }, [token]);

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token) return;
    try {
      await messagesApi.send(token, { recipientUsername: recipient, subject, body });
      alert("Mesaj gönderildi!");
      setRecipient(''); setSubject(''); setBody('');
      fetchInbox(); 
    } catch (err) {
      alert("Hata: " + err);
    }
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (isLoginView) {
        const data = await authApi.login(username, password);
        setToken(data.token);
      } else {
        await authApi.register(username, password);
        alert("Kayıt başarılı! Giriş yapabilirsiniz.");
        setIsLoginView(true);
      }
    } catch (err) {
      alert("İşlem başarısız: " + err);
    }
  };

  const handleLogout = () => {
    setToken(null);
    setUsername('');
    setPassword('');
    setMessages([]);
  };

  return (
    <div id="root">
      <div className="page">
        <header>
          <h1>Confidential Mail</h1>
          {token && (
            <div className="status ok">
              <span>Giriş yapan: <strong>{username}</strong></span>
              <button className="ghost" onClick={handleLogout} style={{ marginLeft: '10px', width: 'auto' }}>Çıkış Yap</button>
            </div>
          )}
        </header>

        <div className="grid">
          {!token ? (
            <div className="panel auth-panel">
               <div className="panel-header">
                <button className={isLoginView ? 'active' : ''} onClick={() => setIsLoginView(true)}>Giriş</button>
                <button className={!isLoginView ? 'active' : ''} onClick={() => setIsLoginView(false)}>Kayıt</button>
              </div>
              <form className="stack" onSubmit={handleLogin}>
                <label>Kullanıcı Adı <input value={username} onChange={(e) => setUsername(e.target.value)} required /></label>
                <label>Parola <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required /></label>
                <button type="submit">{isLoginView ? 'Giriş Yap' : 'Kayıt Ol'}</button>
              </form>
            </div>
          ) : (
            <>
              <div className="panel">
                <h3>Mesaj Gönder</h3>
                <form className="stack" onSubmit={handleSendMessage}>
                  <label>Alıcı: <input value={recipient} onChange={(e) => setRecipient(e.target.value)} required /></label>
                  <label>Konu: <input value={subject} onChange={(e) => setSubject(e.target.value)} required /></label>
                  <label>Mesaj: <textarea value={body} onChange={(e) => setBody(e.target.value)} required /></label>
                  <button type="submit">Gönder</button>
                </form>
              </div>

              <div className="panel">
                <div className="panel-header">
                  <h3>Gelen Kutusu</h3>
                  <button className="ghost" onClick={fetchInbox} style={{width:'auto'}}>Yenile</button>
                </div>
                <div className="messages">
                  {messages.length === 0 ? <p className="muted">Henüz mesaj yok.</p> : messages.map((m) => (
                    <div key={m.messageId} className="message-card">
                      <div className="meta">
                        <span className="pill">Kimden: {m.sender}</span>
                        <span className="pill">Konu: {m.subject}</span>
                      </div>
                      <p>{m.body}</p>
                      <div style={{fontSize: '0.75rem', marginTop: '8px', color: m.signatureValid ? '#22c55e' : '#ef4444'}}>
                        {m.signatureValid ? "🛡️ İmza Doğrulandı" : "⚠️ Geçersiz İmza"}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}