import { useState, useEffect, createElement, useRef } from "react";
import './Igrice.css';
import './Publisher.js'
import Publisher from "./Publisher.js";

export default function Igrice() {
  const [predloziData, setPredloziData] = useState([]);
  const [publisherData, setPublisherData] = useState([]);

  useEffect(() => {
    const gameId = localStorage.getItem("gameId");

    async function predloziFetch() {
      const predlozi = await fetch(
        "http://localhost:5163/api/Recommendation/GetRecommendations?game1Id=" + gameId
      );

      if (!predlozi.ok) {
        return [];
      }

      const data = await predlozi.json();
      return data;
    }

    predloziFetch().then((data) => {
      setPredloziData(data);
    });
  }, [localStorage.getItem("gameId")]);

  return (
    <div>
      <div className="reccomendedGames">
        {predloziData.map((game, index) => {
          return (
            <div className="reccomendedGame" key={index}>
              <img src={game.properties.thumbnail} className="gameImage"/>
              <label>{game.properties.name}</label>
              <Publisher gameId={game.id}/>
              <label>Rating: {game.properties.rating}</label>
            </div>
          )
        })}
      </div>
    </div>
  )
}