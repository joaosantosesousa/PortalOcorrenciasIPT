using Microsoft.AspNetCore.SignalR;

namespace PortalOcorrenciasIPT.Hubs;

// Hub SignalR usado para enviar notificações em tempo real aos clientes ligados.
// Neste projeto, é utilizado para avisar a página de ocorrências quando uma nova ocorrência é criada.
public class OcorrenciasHub : Hub
{
}