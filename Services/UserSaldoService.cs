﻿using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static EFENGSI_RAHMANTO_ZALUKHU.Models.GeneralStatus;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class UserSaldoService : IUserSaldo
    {
        private readonly ApplicationContext _context;

        public UserSaldoService (ApplicationContext conteks)
        {
           _context = conteks;
        }

        public List<UserSaldoDTO> GetListUserSaldo()
        {
            var data =_context.UserSaldos.Include(y => y.User).Select(x => new UserSaldoDTO
            {
                Id = x.Id,
                UserName = x.User.Name,
                Saldo = x.Saldo,

            }).ToList();
            return data;

        }

        public UserSaldo GetUserSaldoById(int id)
        {
            var data =_context.UserSaldos.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return new UserSaldo();
            }

            return data;
        }

        public bool EditUserSaldo(UserSaldoDTO userSaldoDTO)
        {
            var data =_context.UserSaldos.FirstOrDefault(x => x.Id == userSaldoDTO.Id);
            if (data == null)
            {
                return false;
            }
            data.UserId = userSaldoDTO.UserId;
            data.Saldo = userSaldoDTO.Saldo;
            data.LastUpdated = DateTime.Now;
           
           _context.UserSaldos.Update(data);
           _context.SaveChanges();
            return true;
        }

        public bool DeleteUserSaldo(int id)
        {
            var data =_context.UserSaldos.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

           _context.UserSaldos.Remove(data);
           _context.SaveChanges();
            return true;
        }

        public bool AddUserSaldo(UserSaldoDTO userSaldoDTO)
        {
            
            var data = new UserSaldo();

            data.UserId = userSaldoDTO.UserId;
            data.Saldo = userSaldoDTO.Saldo;
            data.LastUpdated = DateTime.Now;
            
           _context.UserSaldos.Add(data);
           _context.SaveChanges();
            return true;

        }


    }
}
