using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using conffandauthh.Models;

namespace conffandauthh.Controllers
{
    public class ChatHistoriesController : Controller
    {
        private conferenceEntities2 db = new conferenceEntities2();

        // GET: ChatHistories
        public ActionResult Index()
        {
            var chatHistory = db.ChatHistory.Include(c => c.OldRooms);
            
            return View(chatHistory.ToList());
        }

        // GET: ChatHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatHistory chatHistory = db.ChatHistory.Find(id);
            if (chatHistory == null)
            {
                return HttpNotFound();
            }
            return View(chatHistory);
        }

        // GET: ChatHistories/Create
        public ActionResult Create()
        {
            ViewBag.oldRoomId = new SelectList(db.OldRooms, "oldRoomId", "ownerId");
            return View();
        }

        // POST: ChatHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "messageId,oldRoomId,userId,content")] ChatHistory chatHistory)
        {
            if (ModelState.IsValid)
            {
                db.ChatHistory.Add(chatHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.oldRoomId = new SelectList(db.OldRooms, "oldRoomId", "ownerId", chatHistory.oldRoomId);
            return View(chatHistory);
        }

        // GET: ChatHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatHistory chatHistory = db.ChatHistory.Find(id);
            if (chatHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.oldRoomId = new SelectList(db.OldRooms, "oldRoomId", "ownerId", chatHistory.oldRoomId);
            return View(chatHistory);
        }

        // POST: ChatHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "messageId,oldRoomId,userId,content")] ChatHistory chatHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.oldRoomId = new SelectList(db.OldRooms, "oldRoomId", "ownerId", chatHistory.oldRoomId);
            return View(chatHistory);
        }

        // GET: ChatHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatHistory chatHistory = db.ChatHistory.Find(id);
            if (chatHistory == null)
            {
                return HttpNotFound();
            }
            return View(chatHistory);
        }

        // POST: ChatHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChatHistory chatHistory = db.ChatHistory.Find(id);
            db.ChatHistory.Remove(chatHistory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
