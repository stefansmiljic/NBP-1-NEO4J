import logo from './logo.svg';
import './App.css';
import { useState, useEffect, createElement } from "react";
import ReactDOM from "react-dom";
import Select from 'react-select';
import Igrice from './Igrice';
import {Route, Routes, BrowserRouter} from "react-router-dom";
import Admin from './Admin';

function App() {
  const [igriceData, setIgriceData] = useState([]);
  const [selectedGame, setSelectedGame] = useState(null);
  const [gameId, setGameId] = useState("");

  const handleChange = (selectedOption) => {
    setSelectedGame(selectedOption);
    setGameId(selectedOption.value);
    localStorage.setItem("gameId", selectedOption.value);
  };

  async function getAllGames() {
    try {
        const data = await fetch("http://localhost:5163/api/Game/GetAllGames", {
            method: "GET",
            mode: 'cors',
        });

        if (!data.ok) {
            throw new Error(`HTTP error! Status: ${data.status}`);
        }

        const returnData = await data.json();
        return returnData;
    } catch (error) {
        console.error("Greška prilikom dohvatanja podataka:", error);
        throw error;
    }
}

  useEffect(() => {
    getAllGames().then((data) => {
      setIgriceData(data);
    });
  }, []);

let igrice = [
  ...igriceData.map((igra) => ({ value: igra.id, label: igra.properties.name })),
];

  return (
    <div className="App">
      <header className="header">
        <h1><a href="#">What2play</a></h1>
      </header>
      
      <h2>Select a Game</h2>
        <Select className="selectGame"
          value={selectedGame}
          onChange={handleChange}
          options={igrice}
          isSearchable
          placeholder="Search for a game..."
        />
        {selectedGame && (
          <div>
            <p style={{color: 'white'}} className="selectedGameName">{selectedGame.label} related games are:</p>
            <Igrice />
          </div>
        )}
        
    </div>
  );

}
export default App;