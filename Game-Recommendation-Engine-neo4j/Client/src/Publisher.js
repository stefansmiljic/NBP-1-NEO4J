import { useState, useEffect, createElement, useRef } from "react";

export default function Publisher(gameId) {
    const [publisherData, setPublisherData] = useState([]);

        async function publisherFetch() {
            var jsonString = JSON.stringify(gameId);
            var parsedObj = JSON.parse(jsonString);
            var gameId2 = parsedObj.gameId;
            var intValue = parseInt(gameId2);
          const publisher = await fetch(
            "http://localhost:5163/api/Game/GetGamePublisher?gameId=" + intValue
          );
        
          if (!publisher.ok) {
            return [];
          }
          
          return publisher.text();
        }
    useEffect(() => {
        publisherFetch().then((data) => {
            setPublisherData(data);
        });
      }, [gameId]);
      return (
        <div className="publisher">
      {publisherData ? (
        <label>{publisherData}</label>
      ) : (
        <label>No publisher data available</label>
      )}
    </div>
      )
}