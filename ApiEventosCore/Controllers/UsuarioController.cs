﻿using ApiEventosCore.Data;
using ApiEventosCore.Models;
using ApiEventosCore.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiEventosCore.Controllers
{
    public class UsuarioController : Controller
    {
        [HttpGet]
        [Authorize]
        public IEnumerable<Usuario> ObterTodos()
        {
            using (var ctx = new EventosDataContext())
            {
                return ctx.Usuario.ToList();
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<Usuario> BuscarPorId(int id)
        {
            using (var ctx = new EventosDataContext())
            {
                return await ctx.Usuario.FindAsync(id);
            }
        }

        [HttpPost]
        public async void Novo([FromBody]Usuario evento)
        {
            using (var ctx = new EventosDataContext())
            {
                evento.Senha = EncryptPassword.Encode(evento.Senha);
                await ctx.Usuario.AddAsync(evento);
                await ctx.SaveChangesAsync();
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async void Delete(int id)
        {
            using (var ctx = new EventosDataContext())
            {
                ctx.Usuario.Remove(await ctx.Usuario.FindAsync(id));
                await ctx.SaveChangesAsync();
            }
        }
    }
}
